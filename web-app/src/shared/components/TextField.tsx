"use client";

import {
	TextField as AriaTextField,
	type TextFieldProps as AriaTextFieldProps,
} from "react-aria-components";
import FieldError from "./FieldError";
import Input from "./Input";
import Label from "./Label";
import Text from "./Text";

interface TextFieldProps extends Omit<AriaTextFieldProps, "className"> {
	label: string;
	description?: string;
	errorMessage?: string;
	labelVariant?: "field" | "group";
	className?: string;
	inputClassName?: string;
}

/** Text field with its label, hint and validation error. */
const TextField = ({
	label,
	description,
	errorMessage,
	labelVariant,
	className = "",
	inputClassName,
	...props
}: Readonly<TextFieldProps>) => {
	return (
		<AriaTextField {...props} className={`grid gap-1.5 ${className}`}>
			<Label variant={labelVariant}>{label}</Label>
			<Input className={inputClassName} />
			{description && (
				<Text slot="description" variant="hint">
					{description}
				</Text>
			)}
			<FieldError>{errorMessage}</FieldError>
		</AriaTextField>
	);
};

export default TextField;
