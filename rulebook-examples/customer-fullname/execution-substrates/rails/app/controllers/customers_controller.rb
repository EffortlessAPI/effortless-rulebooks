class CustomersController < ApplicationController
  before_action :set_customer_for_view, only: [:show]
  before_action :set_customer_for_edit, only: [:edit, :update, :destroy]

  def index
    @customers = Customer.all
  end

  def show
  end

  def new
    @customer = CustomerRaw.new
  end

  def create
    @customer = CustomerRaw.new(customer_params)
    if @customer.save
      redirect_to Customer.find(params[:customer][:customer_id]), notice: 'Customer was successfully created.'
    else
      render :new
    end
  end

  def edit
  end

  def update
    if @customer.update(customer_params)
      redirect_to Customer.find(@customer.customer_id), notice: 'Customer was successfully updated.'
    else
      render :edit
    end
  end

  def destroy
    @customer.destroy
    redirect_to customers_url, notice: 'Customer was successfully destroyed.'
  end

  private

  def set_customer_for_view
    @customer = Customer.find(params[:id])
  end

  def set_customer_for_edit
    @customer = CustomerRaw.find(params[:id])
  end

  def customer_params
    params.require(:customer).permit(:customer_id, :first_name, :last_name, :email_address)
  end
end
