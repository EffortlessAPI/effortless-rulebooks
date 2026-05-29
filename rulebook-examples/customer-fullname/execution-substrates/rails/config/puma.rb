# Puma configuration for Rails app
# React/Node.js app uses 7001/7002 (completely separate)
# Rails uses 4801

port ENV.fetch("PORT") { 4801 }
