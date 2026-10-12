schema "financial_wellness" {
  comment = "User's financial wellness schema"
}

enum "currency_code" {
  schema = schema.financial_wellness
  values = ["USD", "EUR", "CAD", "UAH", "PLN"]
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
