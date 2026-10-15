schema "financial_wellness" {
  comment = "User's financial wellness schema"
}

enum "currency_code" {
  schema = schema.financial_wellness
  values = ["USD", "EUR", "CAD", "UAH", "PLN"]
}

enum "calendar_month" {
  schema = schema.financial_wellness
  values = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"]
}

table "fx_rates" {
  schema  = schema.financial_wellness
  comment = "Daily exchange rates of currencies against the base currency (USD)"

  column "currency" {
    type    = enum.currency_code
    comment = "Currency the rate is quoted for"
  }
  column "units_per_base" {
    type    = numeric(18, 8)
    comment = "Amount of the currency for one unit of the base currency (USD)"
  }
  column "rate_date" {
    type    = date
    default = sql("current_date")
    comment = "Date the rate is valid for"
  }

  primary_key {
    columns = [column.currency, column.rate_date]
  }

  check "fx_rates_units_per_base_positive" {
    expr = "units_per_base > 0"
  }
  check "fx_rates_units_per_base_max" {
    expr = "units_per_base <= 1000000000"
  }
  check "fx_rates_usd_rate_is_one" {
    expr = "currency <> 'USD' OR units_per_base = 1"
  }
}

table "snapshots" {
  schema  = schema.financial_wellness
  comment = "Monthly snapshots of a user's finances: the entered amounts and the index calculated from them"

  column "id" {
    type    = uuid
    default = sql("uuidv7()")
    comment = "Snapshot identifier"
  }
  column "user_id" {
    type    = uuid
    comment = "Owning user identifier"
  }
  column "month" {
    type    = enum.calendar_month
    comment = "Month of the snapshot"
  }
  column "year" {
    type    = smallint
    comment = "Year of the snapshot"
  }
  column "fx_date" {
    type    = date
    comment = "Date of the exchange rates the amounts are converted at"
  }
  column "self_rating" {
    type    = smallint
    comment = "Self-rated financial calm (1-5)"
  }
  column "savings_rate" {
    type    = double_precision
    comment = "Savings rate (SR), clamped to its range"
  }
  column "reserve_months" {
    type    = double_precision
    comment = "Liquid reserves in months of spending (R), clamped to its range"
  }
  column "debt_to_income" {
    type    = double_precision
    comment = "Debt payments to income (DTI), clamped to its range"
  }
  column "expense_variation" {
    type    = double_precision
    comment = "Coefficient of variation of expenses (CV), clamped to its range"
  }
  column "savings_regularity" {
    type    = double_precision
    comment = "Share of recent months with savings (RG)"
  }
  column "cash_flow" {
    type    = double_precision
    comment = "Output of the cash flow subsystem (0-100)"
  }
  column "buffer" {
    type    = double_precision
    comment = "Output of the financial buffer subsystem (0-100)"
  }
  column "i_fuzzy" {
    type    = double_precision
    comment = "Fuzzy index (0-100)"
  }
  column "i_lin" {
    type    = double_precision
    comment = "Linear index for comparison, not shown to the user (0-100)"
  }
  column "i_final" {
    type    = double_precision
    comment = "Final index corrected by the self-rating (0-100)"
  }
  column "amounts" {
    type    = jsonb
    comment = "Entered amounts, each in the currency it was entered in"
  }
  column "model_version" {
    type    = varchar(16)
    comment = "Version of the index model the results were calculated with"
  }
  column "created_at" {
    type    = timestamptz
    default = sql("now()")
    comment = "When the snapshot was created"
  }
  column "updated_at" {
    type    = timestamptz
    default = sql("now()")
    comment = "When the snapshot was last changed"
  }

  primary_key {
    columns = [column.id]
  }

  index "snapshots_user_id_year_month_key" {
    unique  = true
    columns = [column.user_id, column.year, column.month]
  }

  foreign_key "snapshots_user_id_fkey" {
    columns     = [column.user_id]
    ref_columns = [table.users.column.id]
    on_delete   = CASCADE
    on_update   = NO_ACTION
  }
}

table "estimated_snapshots" {
  schema  = schema.financial_wellness
  comment = "Rough months entered at onboarding before the first snapshot. Each new snapshot replaces the oldest one"

  column "id" {
    type    = uuid
    default = sql("uuidv7()")
    comment = "Estimated snapshot identifier"
  }
  column "user_id" {
    type    = uuid
    comment = "Owning user identifier"
  }
  column "month" {
    type    = enum.calendar_month
    comment = "Month of the estimated snapshot"
  }
  column "year" {
    type    = smallint
    comment = "Year of the estimated snapshot"
  }
  column "fx_date" {
    type    = date
    comment = "Date of the exchange rates the amounts are converted at"
  }
  column "expenses_currency" {
    type    = enum.currency_code
    comment = "Currency the total expenses were entered in"
  }
  column "expenses_amount" {
    type    = numeric(15, 2)
    comment = "Total expenses of the month"
  }
  column "saved_currency" {
    type    = enum.currency_code
    comment = "Currency the saved amount was entered in"
  }
  column "saved_amount" {
    type    = numeric(15, 2)
    comment = "Amount saved in the month"
  }
  column "created_at" {
    type    = timestamptz
    default = sql("now()")
    comment = "When the estimated snapshot was created"
  }
  column "updated_at" {
    type    = timestamptz
    default = sql("now()")
    comment = "When the estimated snapshot was last changed"
  }

  primary_key {
    columns = [column.id]
  }

  index "estimated_snapshots_user_id_year_month_key" {
    unique  = true
    columns = [column.user_id, column.year, column.month]
  }

  foreign_key "estimated_snapshots_user_id_fkey" {
    columns     = [column.user_id]
    ref_columns = [table.users.column.id]
    on_delete   = CASCADE
    on_update   = NO_ACTION
  }
}
