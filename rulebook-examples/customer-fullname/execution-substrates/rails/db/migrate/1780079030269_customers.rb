class CreateCustomers < ActiveRecord::Migration[6.1]
  def change
    create_table :customers, primary_key: :customer_id, id: false do |t|
      t.string :customer_id, primary_key: true
      t.string :email_address
      t.string :first_name
      t.string :last_name
    end
  end
end
