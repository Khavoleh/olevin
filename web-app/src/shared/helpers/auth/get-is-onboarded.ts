import { ONBOARDED_COOKIE } from "@shared/constants";
import type { NextRequest } from "next/server";

/** Checks whether the user has completed the onboarding. */
export const getIsOnboarded = (request: NextRequest): boolean =>
	request.cookies.has(ONBOARDED_COOKIE.NAME);
