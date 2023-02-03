import React from 'react'
import { useEffect, useState } from 'react'
import TaskList from './TaskList'
function SprintList(props) {

    const [sprints, setSprints] = useState([])

    useEffect(() => {

        const request = { 
            method: 'GET',
            headers: { 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
        }

        if(sprints.length == 0)
            fetch('https://localhost:7013/Sprint/GetSprints/' + props.teamID, request).then( response => {
                if(response.ok)
                    response.json().then(sprints => {
                        setSprints(sprints)
                    })
        })

    }, [sprints])
    
    const test = () => {setSprints([])}

    return (
        <div className='sprintlist'>
            {sprints.map((sprint) => 
                {
                    if(sprint.status==props.status)                     
                        return  (
                                <div key={sprint.id} className ="sprint">
                                    
                                    <p>{new Date(sprint.startSprint).toDateString()} - {new Date(sprint.endSprint).toDateString()}</p> 
                                    <h1>{sprint.opis}</h1>
                                    <TaskList teamID = {props.teamID} isLeader = {props.isLeader} tasks = {sprint.taskovi} sprintStatus = {sprint.status} test = {test}/>
                                
                                </div>
                                )
                }
            )}
        
        </div>
    )
}

export default SprintList