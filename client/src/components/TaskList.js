import React from 'react'
import { useEffect, useState } from 'react'
import CreateTask from './CreateTask'
import Task from './Task'
function TaskList(props) {

    //const [tasks, setTasks] = useState([])
    const [todo, setTodo] = useState([])
    const [inProgress, setInProgress] = useState([])
    const [underReview, setUnderReview] = useState([])
    const [done, setDone] = useState([])
    const [createTask, setCreateTask] = useState(false)
    const [isLeader, setIsLeader] = useState(false)

    useEffect(() => {

        let todoarr = []
        let inprog = []
        let undreview = []
        let donet = [] 

        //mora se proveriti da li task zapravo pripada trentnom korisniku

        props.tasks.forEach((task) => {
            let obj = {id: task.id, ime: task.ime, opis: task.opis, status: task.status}
            obj =   {   ...obj,
                        ...(task.korisnik != null && {korID: task.korisnik.id, korIme: task.korisnik.username})
                    }

            switch(task.status)
            {
                case 1:
                    todoarr.push(obj)
                    break
                case 2:
                    inprog.push(obj)
                    break
                case 3:
                    undreview.push(obj)
                    break
                case 4:
                    donet.push(obj)
                    break

            }
        })
        setTodo(todoarr)
        setInProgress(inprog)
        setUnderReview(undreview)
        setDone(donet)
        setIsLeader(props.isLeader)
              
        return () => {
   
        }

    }, [props.isLeader])

    //Props za task component pustiti kao obj a ne pojedinacno
    //                {isLeader && <button onClick={() => setCreateTask(!createTask)}> Create Task </button>}

    //{createTask && <CreateTask token={props.token} teamID={props.teamID}/>}
    return (
            <div>
                <h3>To Do:</h3>
                {todo.map(task => (<Task key = {task.id} isLeader={isLeader} taskID = {task.id} ime = {task.ime} opis = {task.opis} status = {task.status} korisnik = {task.korIme} sprintStatus = {props.sprintStatus} test = {props.test}/>))}
                <h3>In Progress:</h3>
                {inProgress.map(task => (<Task key = {task.id} isLeader={isLeader} taskID = {task.id} ime = {task.ime} opis = {task.opis} status = {task.status} korisnik = {task.korIme} sprintStatus = {props.sprintStatus} test = {props.test}/>))}
                <h3>Under Review:</h3>
                {underReview.map(task => (<Task key = {task.id} isLeader={isLeader} taskID = {task.id} ime = {task.ime} opis = {task.opis} status = {task.status} korisnik = {task.korIme} sprintStatus = {props.sprintStatus} test = {props.test}/>))}
                <h3>Done:</h3>
                {done.map(task => (<Task key = {task.id} isLeader={isLeader} taskID = {task.id} ime = {task.ime} opis = {task.opis} status = {task.status} korisnik = {task.korIme} sprintStatus = {props.sprintStatus} test = {props.test}/>))}
            </div>
    )

}

export default TaskList