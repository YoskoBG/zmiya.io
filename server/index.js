const express = require('express');
const http = require('http');
const { WebSocketServer } = require('ws');
const { v4: uuidv4 } = require('uuid');

const TICK_RATE = 30;
const WORLD_SIZE = 3000;
const PLAYER_START_LENGTH = 20;
const FOOD_COUNT = 200;
const FOOD_VALUE = 5;
const MAX_SPEED = 3.5;
const TURN_RATE = 0.15;

const app = express();
app.use(express.static('public'));

const server = http.createServer(app);
const wss = new WebSocketServer({ server });

const players = new Map();
const food = [];

function randomCoord() {
  return Math.random() * WORLD_SIZE - WORLD_SIZE / 2;
}

function spawnFood(count = 1) {
  for (let i = 0; i < count; i++) {
    food.push({
      id: uuidv4(),
      x: randomCoord(),
      y: randomCoord(),
      color: `hsl(${Math.random() * 360}, 70%, 60%)`,
    });
  }
}

function spawnPlayer() {
  return {
    id: uuidv4(),
    x: randomCoord(),
    y: randomCoord(),
    angle: Math.random() * Math.PI * 2,
    speed: 2,
    segments: Array.from({ length: PLAYER_START_LENGTH }, (_, i) => ({
      x: 0,
      y: -i * 8,
    })),
    color: `hsl(${Math.random() * 360}, 65%, 55%)`,
    alive: true,
    score: 0,
  };
}

function stepPlayer(player, input) {
  if (!player.alive) return;
  if (input.turn) {
    player.angle += input.turn * TURN_RATE;
  }
  if (input.boost) {
    player.speed = Math.min(MAX_SPEED, player.speed + 0.02);
  } else {
    player.speed = Math.max(1.8, player.speed - 0.015);
  }

  const dx = Math.cos(player.angle) * player.speed;
  const dy = Math.sin(player.angle) * player.speed;

  player.x += dx;
  player.y += dy;

  const head = { x: player.x, y: player.y };
  player.segments.unshift(head);
  while (player.segments.length > PLAYER_START_LENGTH + player.score) {
    player.segments.pop();
  }
}

function handleFoodCollision(player) {
  for (let i = food.length - 1; i >= 0; i -= 1) {
    const pellet = food[i];
    const dist2 = (pellet.x - player.x) ** 2 + (pellet.y - player.y) ** 2;
    if (dist2 < 25 ** 2) {
      food.splice(i, 1);
      player.score += FOOD_VALUE;
      spawnFood(1);
    }
  }
}

function handlePlayerCollision(player) {
  for (const other of players.values()) {
    if (!other.alive) continue;
    if (other.id === player.id) {
      // Skip the first few segments to avoid self-colliding immediately.
      for (let i = 4; i < player.segments.length; i += 1) {
        const segment = player.segments[i];
        const dist2 = (segment.x - player.x) ** 2 + (segment.y - player.y) ** 2;
        if (dist2 < 14 ** 2) {
          player.alive = false;
          break;
        }
      }
      continue;
    }

    for (const segment of other.segments) {
      const dist2 = (segment.x - player.x) ** 2 + (segment.y - player.y) ** 2;
      if (dist2 < 14 ** 2) {
        player.alive = false;
        break;
      }
    }
  }
}

function broadcastState() {
  const payload = {
    type: 'state',
    players: Array.from(players.values()).map((p) => ({
      id: p.id,
      x: p.x,
      y: p.y,
      angle: p.angle,
      segments: p.segments.slice(0, 80),
      color: p.color,
      alive: p.alive,
      score: p.score,
    })),
    food,
    worldSize: WORLD_SIZE,
  };

  const data = JSON.stringify(payload);
  for (const client of wss.clients) {
    if (client.readyState === 1) {
      client.send(data);
    }
  }
}

function gameTick() {
  for (const player of players.values()) {
    stepPlayer(player, player.input || { turn: 0, boost: false });
    handleFoodCollision(player);
    handlePlayerCollision(player);
  }
  broadcastState();
}

wss.on('connection', (ws) => {
  const player = spawnPlayer();
  players.set(player.id, player);

  ws.send(
    JSON.stringify({ type: 'hello', id: player.id, worldSize: WORLD_SIZE })
  );

  ws.on('message', (data) => {
    try {
      const message = JSON.parse(data.toString());
      if (message.type === 'input') {
        const { turn = 0, boost = false } = message;
        player.input = { turn: Math.max(-1, Math.min(1, turn)), boost };
      }
    } catch (err) {
      console.error('Invalid message', err);
    }
  });

  ws.on('close', () => {
    players.delete(player.id);
  });
});

server.listen(process.env.PORT || 3000, () => {
  console.log('Server listening on http://localhost:3000');
});

spawnFood(FOOD_COUNT);
setInterval(gameTick, 1000 / TICK_RATE);
