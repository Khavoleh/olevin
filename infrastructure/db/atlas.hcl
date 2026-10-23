env "local" {
  src = [
    "file://schemas/public.hcl",
    "file://schemas/settings.hcl",
    "file://schemas/financial_wellness.hcl",
  ]
  url     = "postgres://olevin_atlas:olevin_atlas@postgresql:5432/olevin?sslmode=disable"
  dev     = "postgres://olevin_atlas:olevin_atlas@postgresql:5432/atlas?sslmode=disable"
  schemas = ["public", "settings", "financial_wellness"]
}
