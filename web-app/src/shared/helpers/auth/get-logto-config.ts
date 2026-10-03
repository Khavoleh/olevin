import type { LogtoNextConfig } from "@logto/next";
import { z } from "zod";

/** Server environment variables that Logto needs. */
const authEnvSchema = z.object({
  APP_BASE_URL: z.url(),
  LOGTO_ENDPOINT: z.url(),
  LOGTO_APP_ID: z.string().min(1),
  LOGTO_APP_SECRET: z.string().min(1),
  LOGTO_COOKIE_SECRET: z.string().min(32),
  LOGTO_API_RESOURCE: z.url(),
});

/** Config built on first use and reused afterwards. */
let logtoConfig: LogtoNextConfig | undefined;

/**
 * Reads the Logto settings from the server environment.
 */
export function getLogtoConfig(): LogtoNextConfig {
  if (!logtoConfig) {
    const env = authEnvSchema.parse(process.env);

    logtoConfig = {
      endpoint: env.LOGTO_ENDPOINT,
      appId: env.LOGTO_APP_ID,
      appSecret: env.LOGTO_APP_SECRET,
      baseUrl: env.APP_BASE_URL,
      cookieSecret: env.LOGTO_COOKIE_SECRET,
      cookieSecure: env.APP_BASE_URL.startsWith("https://"),
      resources: [env.LOGTO_API_RESOURCE],
    };
  }

  return logtoConfig;
}
