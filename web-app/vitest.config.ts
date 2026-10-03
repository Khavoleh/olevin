import { resolve } from "node:path";
import { defineConfig } from "vitest/config";

export default defineConfig({
	test: {
		include: ["src/**/*.unit.ts"],
	},
	resolve: {
		alias: {
			"@shared": resolve(import.meta.dirname, "./src/shared"),
			"@widgets": resolve(import.meta.dirname, "./src/widgets"),
			"@features": resolve(import.meta.dirname, "./src/features"),
		},
	},
});
