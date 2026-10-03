import { apiFetch } from "@shared/helpers";
import { z } from "zod";

/** The signed-in user as `/me` returns it. */
const meSchema = z.object({
	subject: z.string(),
});

/** Loads the signed-in user from the API with the given access token. */
export function fetchMe(token: string): Promise<z.infer<typeof meSchema>> {
	return apiFetch("/me", meSchema, token);
}
