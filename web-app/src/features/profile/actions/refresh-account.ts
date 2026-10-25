"use server";

import { refreshApiToken } from "@shared/helpers";
import { refresh } from "next/cache";

/** Takes a new access token with the account as changed in Logto and shows the profile with it. */
export async function refreshAccount(): Promise<void> {
	await refreshApiToken();

	refresh();
}
