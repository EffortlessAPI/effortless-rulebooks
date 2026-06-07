require "active_support/core_ext/integer/time"

Rails.application.configure do
  config.cache_classes = false
  config.eager_load = false
  config.consider_all_requests_local = true
  config.action_controller.perform_caching = false
  config.action_mailer.raise_delivery_errors = false
  # Rails does NOT own this schema — postgres-bootstrap/init-db.sh builds the
  # canonical erb_customer_fullname DB (tables + vw_<entity> views) and Rails only
  # reads it. The pending-migration guard would block every page because the
  # bootstrap-built schema isn't recorded in Rails' schema_migrations table.
  config.active_record.migration_error = false
  config.active_record.verbose_query_logs = true
  # No asset pipeline: this demo renders ERB views and loads Bootstrap from a CDN
  # (see app/views/layouts/application.html.erb). sprockets/webpacker are not
  # dependencies, so `config.assets.*` would raise NoMethodError here.
end
