"use client";

import {
	FieldError as AriaFieldError,
	type FieldErrorProps as AriaFieldErrorProps,
	composeRenderProps,
} from "react-aria-components";

/** Validation error of a form field, shown only while the field is invalid. */
const FieldError = ({ className, ...props }: Readonly<AriaFieldErrorProps>) => {
	return (
		<AriaFieldError
			{...props}
			className={composeRenderProps(
				className,
				(customClasses) => `text-danger text-sm ${customClasses ?? ""}`,
			)}
		/>
	);
};

export default FieldError;
