class Customer < ApplicationRecord
  self.table_name = 'vw_customers'
  self.primary_key = 'customer_id'

  def readonly?
    true
  end
end
