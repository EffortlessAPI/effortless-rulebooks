class Customer < ApplicationRecord
  self.table_name = 'customers'
  self.primary_key = 'customer_id'

  attr_accessor :full_name, :initials

  def full_name
    "#{last_name}, #{first_name}"
  end

  def initials
    "#{first_name[0]}#{last_name[0]}".upcase
  end
end
