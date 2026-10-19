"use client";

import {
	NumberField as AriaNumberField,
	type NumberFieldProps as AriaNumberFieldProps,
} from "react-aria-components";
import FieldError from "./FieldError";
import Input from "./Input";
import Label from "./Label";
import Text from "./Text";

interface NumberFieldProps extends Omit<AriaNumberFieldProps, "className"> {
	label: string;
	description?: string;
	errorMessage?: string;
	labelVariant?: "field" | "group";
	className?: string;
	inputClassName?: string;
}

/** Number field with its label, hint and validation error, formatted for the locale. */
const NumberField = ({
	label,
	description,
	errorMessage,
	labelVariant,
	className = "",
	inputClassName,
	...props
}: Readonly<NumberFieldProps>) => {
	return (
		<AriaNumberField {...props} className={`grid gap-1.5 ${className}`}>
			<Label variant={labelVariant}>{label}</Label>
			<Input className={inputClassName} />
			{description && (
				<Text slot="description" variant="hint">
					{description}
				</Text>
			)}
			<FieldError>{errorMessage}</FieldError>
		</AriaNumberField>
	);
};

export default NumberField;
