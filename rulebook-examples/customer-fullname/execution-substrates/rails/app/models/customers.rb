# Customers - Effortless Entity
# This model reads from vw_customers, which materializes all calculated/lookup/aggregation fields.
# The view IS the contract — never re-derive a computed value in application code.

class Customers < ApplicationRecord
  self.table_name = 'vw_customers'
  self.primary_key = 'customer_id'

  # This model is read-only (views are read-only in Rails)
  def readonly?
    true
  end

end
