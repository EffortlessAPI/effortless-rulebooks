class Customer < ApplicationRecord
  self.table_name = 'vw_customers'
  self.primary_key = 'customer_id'

  # Writes go to the base customers table, reads come from the view
  def self.find_for_edit(id)
    CustomerRaw.find(id)
  end
end

class CustomerRaw < ApplicationRecord
  self.table_name = 'customers'
  self.primary_key = 'customer_id'
end
