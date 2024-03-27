const PROXY_CONFIG = [
  {
    context: [
      "/api/convertor",
    ],
    target: "https://localhost:7126",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
