locals {
  src = [
    "file://schemas/public.hcl",
    "file://schemas/settings.hcl",
    "file://schemas/financial_wellness.hcl",
  ]
  schemas = ["public", "settings", "financial_wellness"]
}

env "local" {
  src     = local.src
  url     = "postgres://olevin_atlas:olevin_atlas@postgresql:5432/olevin?sslmode=disable"
  dev     = "postgres://olevin_atlas:olevin_atlas@postgresql:5432/atlas?sslmode=disable"
  schemas = local.schemas
}

env "production" {
  src     = local.src
  url     = getenv("ATLAS_DATABASE_URL")
  dev     = getenv("ATLAS_DEV_DATABASE_URL")
  schemas = local.schemas

  diff {
    skip {
      drop_schema = true
      drop_table  = true
      drop_column = true
    }
  }
}
