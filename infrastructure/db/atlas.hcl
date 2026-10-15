env "local" {
  src = [
    "file://schemas/public.hcl",
    "file://schemas/outbox.hcl",
    "file://schemas/settings.hcl",
    "file://schemas/financial_wellness.hcl",
  ]
  url = "postgres://olevin:olevin@postgresql:5432/olevin?sslmode=disable"
  dev = "postgres://olevin:olevin@postgresql:5432/atlas?sslmode=disable"
  schemas = ["public", "outbox", "settings", "financial_wellness"]
  exclude = ["outbox.*"] # Wolverine owns every table in the outbox schema.
}
