import React, { useState, useEffect } from 'react';
import './App.css';


const API_URL = 'http://localhost:5000';

function App() {
  const [tasks, setTasks] = useState([]);
  const [newTask, setNewTask] = useState('');
  const [error, setError] = useState('');

  // Fetch all tasks from backend
  const fetchTasks = async () => {
    try {
      const res = await fetch(`${API_URL}/tasks`);
      if (!res.ok) throw new Error('Failed to fetch tasks');
      const data = await res.json();
      setTasks(data);
    } catch (err) {
      setError(err.message);
    }
  };

  useEffect(() => {
    fetchTasks();
  }, []);

  // Add new task
  const addTask = async (e) => {
    e.preventDefault();
    if (!newTask.trim()) return;

    try {
      const res = await fetch(`${API_URL}/tasks`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ task: newTask }),
      });
      if (!res.ok) throw new Error('Failed to add task');
      setNewTask('');
      fetchTasks();
    } catch (err) {
      setError(err.message);
    }
  };

  // Delete task by ID
  const deleteTask = async (id) => {
    try {
      await fetch(`${API_URL}/tasks/${id}`, { method: 'DELETE' });
      fetchTasks();
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div style={{ padding: 20 }}>
      <h1>To-Do List</h1>

      {error && <p style={{ color: 'red' }}>{error}</p>}

      <form onSubmit={addTask}>
        <input
          type="text"
          placeholder="Enter task"
          value={newTask}
          onChange={(e) => setNewTask(e.target.value)}
          required
        />
        <button type="submit">Add Task</button>
      </form>

      <ul>
        {tasks.map((t) => (
          <li key={t._id}>
            {t.task}
            {t.createdAt && ` – ${new Date(t.createdAt).toLocaleString()}`}
            <button
              onClick={() => deleteTask(t._id)}
              style={{ marginLeft: 10 }}
            >
              Delete
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;
