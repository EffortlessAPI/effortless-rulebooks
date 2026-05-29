# Puma configuration for Rails app
# Node.js React app uses 4801 (API) and 4802 (client)
# Rails uses 4803 to avoid conflicts

port ENV.fetch("PORT") { 4803 }
