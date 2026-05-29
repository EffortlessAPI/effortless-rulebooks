require_relative 'boot'
require 'rails/all'

Bundler.require(*Rails.groups)

module CustomerFullname
  class Application < Rails::Application
    config.load_defaults 7.0
    config.api_only = false

    config.generators do |g|
      g.orm :active_record
    end
  end
end
