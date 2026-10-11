schema "financial_wellness" {
  comment = "User's financial wellness schema"
}

enum "currency_code" {
  schema = schema.financial_wellness
  values = ["USD", "EUR", "CAD", "UAH", "PLN"]
}

table "fx_rates" {
  schema  = schema.financial_wellness
  comment = "Daily exchange rates of currencies to the base currency (USD)"

  column "currency" {
    type    = enum.currency_code
    comment = "Currency the rate is quoted for"
  }
  column "rate_to_base" {
    type    = numeric(18, 8)
    comment = "Amount of base currency (USD) for one unit of currency"
  }
  column "rate_date" {
    type    = date
    default = sql("current_date")
    comment = "Date the rate is valid for"
  }

  primary_key {
    columns = [column.currency, column.rate_date]
  }

  check "fx_rates_rate_to_base_positive" {
    expr = "rate_to_base > 0"
  }
  check "fx_rates_rate_to_base_max" {
    expr = "rate_to_base <= 1000000"
  }
  check "fx_rates_usd_rate_is_one" {
    expr = "currency <> 'USD' OR rate_to_base = 1"
  }
}
