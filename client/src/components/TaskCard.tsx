import { Task } from '../utils/types';

const TaskCard = ({
  task,
  updateTask,
}: {
  task: Task;
  updateTask: (task: Task) => void;
}) => {
  return (
    <div
      draggable
      onDragStart={(e) => {
        e.dataTransfer.setData('id', task.id);
      }}
      className="w- m-2 h-20 rounded-lg border bg-white px-2"
    >
      <div className="font-base py-2 text-base">
        <div>{task.name}</div>
        <div>{task.contractor}</div>
      </div>
    </div>
  );
};

export default TaskCard;
