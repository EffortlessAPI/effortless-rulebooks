Rails.application.routes.draw do
  root 'customers#index'
  resources :customers
end
