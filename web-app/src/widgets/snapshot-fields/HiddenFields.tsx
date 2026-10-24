import { getFormEntries } from "./helpers/get-form-entries/get-form-entries";

interface HiddenFieldsProps {
	value: object;
}

/** Hidden inputs that send the object with the form. */
const HiddenFields = ({ value }: Readonly<HiddenFieldsProps>) =>
	getFormEntries(value).map(([name, entry]) => (
		<input key={name} type="hidden" name={name} value={entry} />
	));

export default HiddenFields;
