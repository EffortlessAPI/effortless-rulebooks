export type Me = {
  user_id: string;
  email: string;
  role: "trainer" | "client" | "admin";
  display_name: string;
  trainer_id?: string | null;
  client_id?: string | null;
};

export type DevUser = {
  user_id: string;
  email: string;
  role: "trainer" | "client" | "admin";
  display_name: string;
};

export type Invoice = {
  invoice_id: string;
  name: string;
  client: string;
  client_name: string;
  client_email: string;
  trainer: string;
  trainer_name: string;
  issue_date: string;
  due_date: string;
  tax_rate: string | number;
  discount_amount: string | number;
  paid_amount: string | number;
  subtotal: string | number;
  tax_amount: string | number;
  days_past_due: number;
  late_fee: string | number;
  total: string | number;
  balance: string | number;
  is_paid: boolean;
  is_overdue: boolean;
  status: "Paid" | "Open" | "Overdue";
};

export type Session = {
  session_id: string;
  name: string;
  client: string;
  client_name: string;
  trainer: string;
  client_hourly_rate: string | number;
  session_date: string;
  duration_hours: string | number;
  rate_override: string | number;
  notes: string | null;
  effective_rate: string | number;
  line_total: string | number;
  invoice: string | null;
  invoice_name: string | null;
  is_billed: boolean;
};

export type Client = {
  client_id: string;
  name: string;
  email: string;
  full_name: string;
  trainer: string;
  trainer_name: string;
  trainer_email: string;
  trainer_hourly_rate: string | number;
  session_count: number;
  total_invoiced: string | number;
  outstanding_balance: string | number;
  overdue_count: number;
  status: string;
};

export type Trainer = {
  trainer_id: string;
  name: string;
  email: string;
  full_name: string;
  hourly_rate: string | number;
  total_sessions: number;
  total_billed: string | number;
  total_outstanding: string | number;
};
