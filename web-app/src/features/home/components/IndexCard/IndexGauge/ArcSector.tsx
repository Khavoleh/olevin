import { type PieSectorShapeProps, Sector } from "recharts";

/** Arc of the gauge, filled with the token class of its data item. */
const ArcSector = (props: PieSectorShapeProps) => (
	<Sector {...props} className={props.payload.className} />
);

export default ArcSector;
