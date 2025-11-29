# Slither-Style Arena

A minimal WebSocket-powered prototype inspired by **slither.io**. Multiple players can join, steer their snakes, eat pellets to grow, and collide with each other in a shared arena.

## Features
- Node.js + `ws` server that keeps a simple game state and broadcasts it at 30 ticks per second.
- Canvas-based client rendering snakes, pellets, and a lightweight HUD.
- Keyboard controls: Left/Right (or A/D) to steer, Space to boost.

## Getting started

```bash
npm install
npm start
```

Then open http://localhost:3000 in two or more browser windows to see multiplayer movement.

## Notes
- This is an intentionally lightweight clone, not a pixel-perfect recreation of slither.io. Feel free to extend physics, art, and networking as needed.
