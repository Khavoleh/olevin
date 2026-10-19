interface ApiResponse {
	status: number;
	data?: unknown;
}

interface ApiEnvelope {
	error?: string | null;
	validationErrors?: string[] | null;
}

const getMessage = ({ status, data }: ApiResponse) => {
	const { error, validationErrors } = (data ?? {}) as ApiEnvelope;

	if (status === 400 && validationErrors?.length) {
		return validationErrors.join(", ");
	}

	return (
		error ||
		`The API responded with status ${status}. Contact the support, please.`
	);
};

/** The API answered with a non-2xx status; the message is the error of the response, or its validation errors joined by commas for 400. */
export class ApiError extends Error {
	readonly status: number;

	constructor(response: ApiResponse) {
		super(getMessage(response));
		this.name = "ApiError";
		this.status = response.status;
	}
}
