const PROXY_CONFIG = [
  {
    context: [
      "/get-game-state",
      "/compare-cards",
      "/register-player-name",
      "/check-game-state",
      "/reset-game-state",
      "/update-cards",
      "/remove-eliminate-card",
      "/remove-reset-game-status"
    ],
    target: "https://localhost:7012",
    secure: false,
    changeOrigin: true,
  }
]

module.exports = PROXY_CONFIG;
