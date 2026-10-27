import { z } from "zod";

/** Server environment variables that the push subscription needs. */
const pushEnvSchema = z.object({
	VAPID_PUBLIC_KEY: z.string().regex(/^[A-Za-z0-9_-]{87}$/),
});

/** Public VAPID key of the API that the browser subscribes with. */
export function getVapidPublicKey(): string {
	return pushEnvSchema.parse(process.env).VAPID_PUBLIC_KEY;
}
