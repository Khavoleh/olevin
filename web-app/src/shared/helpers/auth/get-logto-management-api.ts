import { createManagementApi } from "@logto/api/management";
import { z } from "zod";

/** Server environment variables that the Logto Management API needs. */
const managementEnvSchema = z.object({
	LOGTO_ENDPOINT: z.url(),
	LOGTO_M2M_APP_ID: z.string().min(1),
	LOGTO_M2M_APP_SECRET: z.string().min(1),
});

/** Client built on first use and reused afterwards, so its access token is cached. */
let managementApi: ReturnType<typeof createManagementApi> | undefined;

/**
 * Reads the machine-to-machine application from the server environment and gives the client of the Logto Management API.
 */
export function getLogtoManagementApi(): ReturnType<
	typeof createManagementApi
> {
	if (!managementApi) {
		const env = managementEnvSchema.parse(process.env);

		managementApi = createManagementApi({
			tenantId: "default",
			baseUrl: env.LOGTO_ENDPOINT,
			clientId: env.LOGTO_M2M_APP_ID,
			clientSecret: env.LOGTO_M2M_APP_SECRET,
		});
	}

	return managementApi;
}
