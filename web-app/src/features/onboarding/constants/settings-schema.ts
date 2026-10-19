import { z } from "zod";
import { ConfigureUserSettingsBody } from "../api/onboarding.zod";

/** Settings form, where an unchecked consent box means `false`. */
export const SETTINGS_SCHEMA = ConfigureUserSettingsBody.extend({
	researchConsent: z.boolean().default(false),
});
