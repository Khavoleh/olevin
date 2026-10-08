env "local" {
  src = [
    "file://schemas/public.hcl",
    "file://schemas/settings.hcl",
  ]
  url = "postgres://olevin:olevin@postgresql:5432/olevin?sslmode=disable"
  dev = "postgres://olevin:olevin@postgresql:5432/atlas?sslmode=disable"
  schemas = ["public", "settings"]
}
