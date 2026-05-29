require_relative 'boot'
require 'rails/all'

Bundler.require(*Rails.groups)

module CustomerFullname
  class Application < Rails::Application
    config.load_defaults 7.0
    config.api_only = false

    # Database configuration
    config.database_configuration = {
      'default' => {
        'adapter' => 'postgresql',
        'database' => ENV['DATABASE_URL'] || 'erb_customer_fullname',
        'host' => ENV['DB_HOST'] || 'localhost',
        'port' => ENV['DB_PORT'] || 5432,
        'user' => ENV['DB_USER'] || 'postgres',
        'password' => ENV['DB_PASSWORD'] || ''
      }
    }

    config.generators do |g|
      g.orm :active_record
      g.test_framework :rspec
      g.fixture_replacement :factory_bot, dir: 'spec/factories'
    end
  end
end
