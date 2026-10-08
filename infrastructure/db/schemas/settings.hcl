schema "settings" {
  comment = "Application user's settings schema"
}

enum "currency_code" {
  schema = schema.settings
  values = ["UAH", "USD", "EUR", "CAD", "PLN"]
}

enum "locale_code" {
  schema = schema.settings
  values = ["UK", "EN"]
}

table "users" {
  schema  = schema.settings
  comment = "Application user profile"

  column "id" {
    type    = uuid
    default = sql("uuidv7()")
    comment = "User identifier"
  }
  column "auth_sub" {
    type    = varchar(128)
    comment = "Logto sub claim"
  }
  column "base_currency" {
    type    = enum.currency_code
    comment = "Default user's currency"
  }
  column "locale" {
    type    = enum.locale_code
    comment = "Interface language"
  }
  column "snapshot_day" {
    type    = smallint
    comment = "Day of the month the monthly snapshot is taken on (1-31)"
  }
  column "timezone" {
    type    = varchar(64)
    comment = "IANA time zone name"
  }
  column "created_at" {
    type    = timestamptz
    default = sql("now()")
    comment = "When the profile was created"
  }
  column "updated_at" {
    type    = timestamptz
    default = sql("now()")
    comment = "When the profile was last changed"
  }

  primary_key {
    columns = [column.id]
  }

  index "users_auth_sub_key" {
    unique  = true
    columns = [column.auth_sub]
  }

  check "users_auth_sub_not_blank" {
    expr = "length(btrim(auth_sub)) > 0"
  }
  check "users_snapshot_day_range" {
    expr = "snapshot_day BETWEEN 1 AND 31"
  }
  check "users_updated_at_not_before_created_at" {
    expr = "updated_at >= created_at"
  }
  check "users_timezone_format" {
    expr = "timezone ~ '^[A-Za-z0-9_+-]+(/[A-Za-z0-9_+-]+)*$'"
  }
}
