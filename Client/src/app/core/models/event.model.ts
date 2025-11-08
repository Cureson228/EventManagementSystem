import { Participant } from "./participant.model";

export interface Event {
    id: number;
    title: string;
    description: string;
    location: string;
    dateTime : Date;
    createdByUserId: string;
    capacity: number | null;
    participants: Participant[];
}