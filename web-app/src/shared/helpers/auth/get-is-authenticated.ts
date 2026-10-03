import LogtoClient from "@logto/next/edge";
import type { NextRequest } from "next/server";
import { getLogtoConfig } from "./get-logto-config";

/** Edge Logto client, created on first use and reused afterwards. */
let logtoClient: LogtoClient | undefined;

/**
 * Checks the Logto session cookie of a request. Used by the proxy, so pages stay static.
 */
export async function getIsAuthenticated(
	request: NextRequest,
): Promise<boolean> {
	logtoClient ??= new LogtoClient(getLogtoConfig());

	const { isAuthenticated } = await logtoClient.getLogtoContext(request);

	return isAuthenticated;
}
