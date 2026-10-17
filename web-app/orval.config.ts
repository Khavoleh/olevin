import { defineConfig } from "orval";

const apiUrl = "https://api.localhost";

export default defineConfig(async () => {
  const response = await fetch(`${apiUrl}/openapi/v1.json`);

  if (!response.ok) {
    throw new Error(
      `The API answered ${response.status}. Launch the server, please.`,
    );
  }

  const spec = await response.json();
  const tags = new Set<string>(
    Object.values(spec.paths).flatMap((operations) =>
      Object.values(operations as Record<string, { tags?: string[] }>).flatMap(
        (operation) => operation.tags ?? [],
      ),
    ),
  );

  return Object.fromEntries(
    [...tags].flatMap((tag) => {
      const feature = tag.replace(/([a-z0-9])([A-Z])/g, "$1-$2").toLowerCase();
      const folder = `src/features/${feature}/api`;
      const input = { target: spec, filters: { tags: [tag] } };

      return [
        [
          `${feature}-fetch`,
          {
            input,
            output: {
              client: "fetch",
              mode: "split",
              target: `${folder}/${feature}.ts`,
              formatter: "biome",
              override: {
                mutator: {
                  path: "@shared/helpers",
                  name: "apiRequest",
                },
              },
            },
          },
        ],
        [
          `${feature}-zod`,
          {
            input,
            output: {
              client: "zod",
              target: `${folder}/${feature}.zod.ts`,
              fileExtension: ".zod.ts",
              formatter: "biome",
              override: {
                zod: {
                  version: 4,
                  generate: {
                    param: true,
                    query: true,
                    header: false,
                    body: true,
                    response: false,
                  },
                },
              },
            },
          },
        ],
      ];
    }),
  );
});
