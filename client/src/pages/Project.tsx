import axios from 'axios';
import { useEffect, useState } from 'react';
import { statuses } from '../utils/data-tasks';
import TaskCard from '../components/TaskCard';
import ButtonMain from '../components/ui/ButtonMain';
import { ProjectData, Status, Task } from '../utils/types';

export default function Project() {
  const [data, setData] = useState<ProjectData | undefined>();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [projectId, setProjectId] = useState<string>('');

  useEffect(() => {
    setProjectId(window.location.href.split('/')[4]);
    if (projectId) {
      axios
        .get(`/api/projects/{id}?projectId=${projectId}`)
        .then((res) => {
          setData(res.data.project);
          setTasks(res.data.project.projectTasks);
        })
        .catch((err) => console.error(err));
    }
  }, [projectId]);

  const columns = statuses.map((status) => {
    const tasksInColumn = tasks.filter(
      (task) => statuses[task.taskStatus - 1] === status,
    );
    const statusNumber = statuses.indexOf(status) + 1;
    return {
      status,
      tasks: tasksInColumn,
      statusNumber,
    };
  });

  console.log(columns);

  const updateTask = (task: Task) => {
    const status = task.taskStatus;
    console.log('status ' + status);
    axios.put('/api/projecttasks/status/{id}', {
      projectTaskId: task.id,
      taskStatus: status,
    });

    const updatedTasks = tasks.map((t) => {
      console.log(task);
      return t.id === task.id ? task : t;
    });
    setTasks(updatedTasks);
  };

  const handleDrop = (
    e: React.DragEvent<HTMLDivElement>,
    taskStatus: number,
  ) => {
    e.preventDefault();
    setCurrentlyHoveringOver(null);
    const id = e.dataTransfer.getData('id');
    const task = tasks.find((task) => task.id.toString() === id.toString());
    if (task) {
      updateTask({ ...task, taskStatus });
    }
  };

  const [currentlyHoveringOver, setCurrentlyHoveringOver] =
    useState<Status | null>(null);
  const handleDragEnter = (status: Status) => {
    setCurrentlyHoveringOver(status);
  };

  return data ? (
    <div className="h-screen bg-login bg-cover bg-no-repeat">
      <div className="mx-auto max-w-7xl rounded-lg bg-white px-10 pb-10 ">
        <div className="flex justify-between">
          <div className="mb-5 mt-10 text-2xl font-semibold">
            Project: {data.name}
          </div>
          <ButtonMain text="Create task" type="button" className="mt-14 px-5" />
        </div>

        <div className="relative mb-5">
          {data.participants.map((p) => (
            <div
              key={p.user.fullName}
              className="flex h-10 w-10 items-center justify-center rounded-full bg-blue-500 text-xl font-semibold capitalize"
            >
              {p.user.fullName[0]}
            </div>
          ))}
          <button className="absolute bottom-4 left-12 flex flex-col items-center justify-center">
            <div className="h-1 w-6 bg-blue-500"></div>
            <div className="absolute h-6 w-1 bg-blue-500"></div>
          </button>
        </div>
        <div className="flex gap-5">
          {columns.map((column) => (
            <div
              key={column.statusNumber}
              onDrop={(e) => handleDrop(e, column.statusNumber)}
              onDragOver={(e) => e.preventDefault()}
              onDragEnter={() => handleDragEnter(column.statusNumber)}
            >
              <div className="h-[30rem] w-64 rounded-lg border bg-gray-100 p-2 text-center text-2xl  ">
                <h2 className="mb-6 font-semibold capitalize">
                  {column.status}
                </h2>
                <div
                  className={`h-[85%] ${currentlyHoveringOver === column.statusNumber ? 'bg-gray-200' : ''}`}
                >
                  {column.tasks.map((task) => (
                    <TaskCard
                      key={task.id}
                      task={task}
                      updateTask={updateTask}
                    />
                  ))}
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  ) : (
    <div></div>
  );
}
