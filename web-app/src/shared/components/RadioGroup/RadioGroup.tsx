"use client";

import type { ReactNode } from "react";
import {
	RadioGroup as AriaRadioGroup,
	type RadioGroupProps as AriaRadioGroupProps,
} from "react-aria-components";
import FieldError from "../FieldError";
import Label from "../Label";
import Text from "../Text";

interface RadioGroupProps
	extends Omit<AriaRadioGroupProps, "className" | "children"> {
	label: string;
	description?: string;
	errorMessage?: string;
	labelVariant?: "field" | "group";
	className?: string;
	children: ReactNode;
}

/** Group of mutually exclusive options with its label, hint and validation error. */
const RadioGroup = ({
	label,
	description,
	errorMessage,
	labelVariant = "group",
	className = "",
	children,
	...props
}: Readonly<RadioGroupProps>) => {
	return (
		<AriaRadioGroup {...props} className={`grid gap-2 ${className}`}>
			<Label variant={labelVariant}>{label}</Label>
			{description && (
				<Text slot="description" variant="hint" className="mb-1">
					{description}
				</Text>
			)}
			{children}
			<FieldError>{errorMessage}</FieldError>
		</AriaRadioGroup>
	);
};

export default RadioGroup;
