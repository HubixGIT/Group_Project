export type ProjectData = {
  id: number;
  name: string;
  participants: Participant[];
  projectTasks: ProjectTask[];
};

export type Participant = {
  user: {
    id: string;
    fullName: string;
    email: string;
  };
};

export type ProjectTask = {
  name: string;
  contractor: string | null;
  taskStatus: number;
};

export type Status = number;
export type Task = {
  name: string;
  id: string;
  taskStatus: number;
  contractor?: string;
  comments?: [];
};
