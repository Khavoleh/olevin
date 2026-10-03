import { getLogtoConfig } from "../auth/get-logto-config";

/** Logto resource indicator of the Olevin API, the audience of its access tokens. */
export function getApiResource(): string {
  const [resource] = getLogtoConfig().resources ?? [];

  if (!resource) {
    throw new Error("The Logto API resource is not configured.");
  }

  return resource;
}
