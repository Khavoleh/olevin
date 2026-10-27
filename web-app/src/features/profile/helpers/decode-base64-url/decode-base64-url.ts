/** Bytes of a base64url string, such as the VAPID public key the browser subscribes with. */
export const decodeBase64Url = (value: string): Uint8Array<ArrayBuffer> => {
	const base64 = value.replaceAll("-", "+").replaceAll("_", "/");
	const binary = atob(base64.padEnd(Math.ceil(base64.length / 4) * 4, "="));

	return Uint8Array.from(binary, (char) => char.charCodeAt(0));
};
