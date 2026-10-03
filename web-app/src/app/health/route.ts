/**
 * Liveness check for the container.
 */
export function GET() {
	return new Response("Healthy");
}
