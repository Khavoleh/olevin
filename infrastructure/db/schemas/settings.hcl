schema "settings" {
  comment = "Application user's settings schema"
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
  check "users_updated_at_not_before_created_at" {
    expr = "updated_at >= created_at"
  }
}

table "user_settings" {
  schema  = schema.settings
  comment = "Per-user application settings (one row per user)"

  column "user_id" {
    type    = uuid
    comment = "Owning user identifier"
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
  column "research_consent" {
    type    = boolean
    default = false
    comment = "Whether the user consented to use of their data for research"
  }
  column "created_at" {
    type    = timestamptz
    default = sql("now()")
    comment = "When the settings were created"
  }
  column "updated_at" {
    type    = timestamptz
    default = sql("now()")
    comment = "When the settings were last changed"
  }

  primary_key {
    columns = [column.user_id]
  }

  foreign_key "user_settings_user_id_fkey" {
    columns     = [column.user_id]
    ref_columns = [table.users.column.id]
    on_delete   = CASCADE
    on_update   = NO_ACTION
  }

  check "user_settings_snapshot_day_range" {
    expr = "snapshot_day BETWEEN 1 AND 31"
  }
  check "user_settings_updated_at_not_before_created_at" {
    expr = "updated_at >= created_at"
  }
  check "user_settings_timezone_format" {
    expr = "timezone ~ '^[A-Za-z0-9_+-]+(/[A-Za-z0-9_+-]+)*$'"
  }
}

table "cfpb_responses" {
  schema  = schema.settings
  comment = "CFPB Financial Well-Being Scale answers given once at onboarding"

  column "id" {
    type    = uuid
    default = sql("uuidv7()")
    comment = "Response identifier"
  }
  column "user_id" {
    type    = uuid
    comment = "Owning user identifier"
  }
  column "answers" {
    type    = sql("smallint[]")
    comment = "Response values of the 10 statements in order"
  }
  column "is_62_plus" {
    type    = boolean
    comment = "Whether the respondent is 62 or older"
  }
  column "score" {
    type    = smallint
    comment = "Financial well-being score from the self-administered conversion table"
  }
  column "submitted_at" {
    type    = timestamptz
    default = sql("now()")
    comment = "When the response was submitted"
  }

  primary_key {
    columns = [column.id]
  }

  index "cfpb_responses_user_id_key" {
    unique  = true
    columns = [column.user_id]
  }

  foreign_key "cfpb_responses_user_id_fkey" {
    columns     = [column.user_id]
    ref_columns = [table.users.column.id]
    on_delete   = CASCADE
    on_update   = NO_ACTION
  }

  check "cfpb_responses_answers_shape" {
    expr = "array_ndims(answers) = 1 AND cardinality(answers) = 10 AND array_position(answers, NULL) IS NULL AND 0 <= ALL(answers) AND 4 >= ALL(answers)"
  }
  check "cfpb_responses_score_range" {
    expr = "score BETWEEN 14 AND 95"
  }
}
