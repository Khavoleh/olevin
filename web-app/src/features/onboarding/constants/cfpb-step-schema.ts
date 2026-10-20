import { z } from "zod";
import { SubmitCfpbResponseBody } from "../api/onboarding.zod";

/** CFPB questionnaire form, where the age group comes as the radio value `"true"` or `"false"`. */
export const CFPB_STEP_SCHEMA = SubmitCfpbResponseBody.extend({
	is62Plus: z.enum(["true", "false"]).transform((value) => value === "true"),
});
