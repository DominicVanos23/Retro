const express = require('express');
const cors = require('cors');
const { MongoClient, ObjectId } = require('mongodb');

const app = express();
const port = 5000;

const uri = 'mongodb://localhost:27017';
const client = new MongoClient(uri);
const dbName = 'todoDB';

app.use(cors());
app.use(express.json());

async function main() {
  await client.connect();
  console.log('Connected to MongoDB');
  const db = client.db(dbName);
  const tasks = db.collection('tasks');

  // Add Task
  app.post('/tasks', async (req, res) => {
    const { task } = req.body;
    if (!task) return res.status(400).json({ error: 'Task is required' });

    const newTask = {
      task,
      createdAt: new Date()
    };

    const result = await tasks.insertOne(newTask);
    res.status(201).json(result.ops?.[0] || newTask); // fallback for modern drivers
  });

  // Get All Tasks
  app.get('/tasks', async (req, res) => {
    const allTasks = await tasks.find().toArray();
    res.json(allTasks);
  });

  // Delete Task
  app.delete('/tasks/:id', async (req, res) => {
    const { id } = req.params;
    try {
      const result = await tasks.deleteOne({ _id: new ObjectId(id) });
      if (result.deletedCount === 0) {
        return res.status(404).json({ error: 'Task not found' });
      }
      res.json({ message: 'Task deleted' });
    } catch (err) {
      res.status(500).json({ error: 'Invalid ID format' });
    }
  });

  app.listen(port, () => {
    console.log(`Server running at http://localhost:${port}`);
  });
}

main().catch(console.error);
