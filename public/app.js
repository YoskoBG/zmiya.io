const canvas = document.getElementById('game');
const ctx = canvas.getContext('2d');
const hud = document.querySelector('.stats');

let width = window.innerWidth;
let height = window.innerHeight;
canvas.width = width;
canvas.height = height;

window.addEventListener('resize', () => {
  width = window.innerWidth;
  height = window.innerHeight;
  canvas.width = width;
  canvas.height = height;
});

const state = {
  id: null,
  worldSize: 3000,
  players: [],
  food: [],
  input: { turn: 0, boost: false },
};

const ws = new WebSocket(`ws://${window.location.host}`);

ws.addEventListener('message', (event) => {
  const message = JSON.parse(event.data);
  if (message.type === 'hello') {
    state.id = message.id;
    state.worldSize = message.worldSize;
  }
  if (message.type === 'state') {
    state.players = message.players;
    state.food = message.food;
  }
});

const keys = new Set();
window.addEventListener('keydown', (e) => {
  keys.add(e.code);
});
window.addEventListener('keyup', (e) => {
  keys.delete(e.code);
});

function updateInput() {
  let turn = 0;
  if (keys.has('ArrowLeft') || keys.has('KeyA')) turn -= 1;
  if (keys.has('ArrowRight') || keys.has('KeyD')) turn += 1;
  const boost = keys.has('Space');
  state.input = { turn, boost };
  ws.send(JSON.stringify({ type: 'input', ...state.input }));
}

function drawGrid(camera) {
  const spacing = 60;
  ctx.strokeStyle = 'rgba(255,255,255,0.05)';
  ctx.lineWidth = 1;
  for (let x = -state.worldSize; x <= state.worldSize; x += spacing) {
    const sx = x - camera.x + width / 2;
    ctx.beginPath();
    ctx.moveTo(sx, 0);
    ctx.lineTo(sx, height);
    ctx.stroke();
  }
  for (let y = -state.worldSize; y <= state.worldSize; y += spacing) {
    const sy = y - camera.y + height / 2;
    ctx.beginPath();
    ctx.moveTo(0, sy);
    ctx.lineTo(width, sy);
    ctx.stroke();
  }
}

function drawSnake(player, camera, isSelf) {
  ctx.lineCap = 'round';
  ctx.lineJoin = 'round';
  ctx.lineWidth = 14;
  ctx.strokeStyle = isSelf ? '#00ffb7' : player.color;
  ctx.shadowBlur = 8;
  ctx.shadowColor = ctx.strokeStyle;
  ctx.beginPath();
  player.segments.slice().reverse().forEach((segment, index) => {
    const sx = segment.x - camera.x + width / 2;
    const sy = segment.y - camera.y + height / 2;
    if (index === 0) {
      ctx.moveTo(sx, sy);
    } else {
      ctx.lineTo(sx, sy);
    }
  });
  ctx.stroke();
  ctx.shadowBlur = 0;
}

function drawFood(camera) {
  for (const pellet of state.food) {
    const sx = pellet.x - camera.x + width / 2;
    const sy = pellet.y - camera.y + height / 2;
    ctx.fillStyle = pellet.color;
    ctx.beginPath();
    ctx.arc(sx, sy, 5, 0, Math.PI * 2);
    ctx.fill();
  }
}

function render() {
  ctx.clearRect(0, 0, width, height);
  const me = state.players.find((p) => p.id === state.id);
  const camera = me ? { x: me.x, y: me.y } : { x: 0, y: 0 };
  drawGrid(camera);
  drawFood(camera);
  state.players.forEach((p) => drawSnake(p, camera, p.id === state.id));

  const leaderboard = [...state.players]
    .sort((a, b) => b.score - a.score)
    .slice(0, 5)
    .map((p, i) => `${i + 1}. ${p.id.slice(0, 4)} — ${p.score}`)
    .join('  ');
  hud.textContent = me
    ? `You: ${me.score}  |  Alive: ${me.alive ? 'Yes' : 'No'}  |  ${leaderboard}`
    : 'Connecting...';

  requestAnimationFrame(render);
}

setInterval(updateInput, 1000 / 20);
requestAnimationFrame(render);
